import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize, interval, Subscription, take } from 'rxjs';
import { AuthenticationService } from '../../core/auth/authentication.service';
import { LocalizationService } from '../../localization/localization.service';
import { LanguageSwitcher } from '../../shared/language-switcher/language-switcher';

type SignInMode = 'password' | 'otp';
type OtpStep = 'request' | 'verify';

@Component({
  selector: 'app-login-page',
  imports: [ReactiveFormsModule, LanguageSwitcher],
  templateUrl: './login-page.html',
  styleUrl: './login-page.scss',
})
export class LoginPage {
  private readonly authentication = inject(AuthenticationService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private resendCountdown?: Subscription;

  protected readonly i18n = inject(LocalizationService);
  protected readonly mode = signal<SignInMode>('password');
  protected readonly otpStep = signal<OtpStep>('request');
  protected readonly otpRequestId = signal<string | null>(null);
  protected readonly resendSeconds = signal(0);
  protected readonly isSubmitting = signal(false);
  protected readonly submitFailed = signal(false);

  protected readonly passwordForm = new FormGroup({
    username: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required],
    }),
    password: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required],
    }),
  });

  protected readonly otpRequestForm = new FormGroup({
    phoneNumber: new FormControl('', {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.pattern(/^(?:(?:\+|00)?98|0)?9\d{9}$/),
      ],
    }),
  });

  protected readonly otpCodeForm = new FormGroup({
    code: new FormControl('', {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.pattern(/^\d{4,10}$/),
      ],
    }),
  });

  protected setMode(mode: SignInMode): void {
    if (this.isSubmitting()) return;
    this.mode.set(mode);
    this.submitFailed.set(false);
  }

  protected submitPassword(): void {
    if (this.passwordForm.invalid || this.isSubmitting()) {
      this.passwordForm.markAllAsTouched();
      return;
    }

    this.runSubmission(
      this.authentication.login(this.passwordForm.getRawValue()),
      true,
    );
  }

  protected requestOtp(): void {
    if (this.otpRequestForm.invalid || this.isSubmitting() || this.resendSeconds() > 0) {
      this.otpRequestForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.submitFailed.set(false);
    this.authentication
      .requestOtp(this.otpRequestForm.controls.phoneNumber.value)
      .pipe(
        finalize(() => this.isSubmitting.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (response) => {
          this.otpRequestId.set(response.requestId);
          this.otpStep.set('verify');
          this.otpCodeForm.reset();
          this.startResendCountdown(response.retryAfterSeconds);
        },
        error: () => this.submitFailed.set(true),
      });
  }

  protected verifyOtp(): void {
    const requestId = this.otpRequestId();
    if (this.otpCodeForm.invalid || !requestId || this.isSubmitting()) {
      this.otpCodeForm.markAllAsTouched();
      return;
    }

    this.runSubmission(
      this.authentication.verifyOtp({
        requestId,
        phoneNumber: this.otpRequestForm.controls.phoneNumber.value,
        code: this.otpCodeForm.controls.code.value,
      }),
      true,
    );
  }

  protected changePhoneNumber(): void {
    if (this.isSubmitting()) return;
    this.resendCountdown?.unsubscribe();
    this.otpStep.set('request');
    this.otpRequestId.set(null);
    this.otpCodeForm.reset();
    this.resendSeconds.set(0);
    this.submitFailed.set(false);
  }

  private runSubmission(
    operation: ReturnType<AuthenticationService['login']>,
    navigateAfterSuccess: boolean,
  ): void {
    this.isSubmitting.set(true);
    this.submitFailed.set(false);
    operation
      .pipe(
        finalize(() => this.isSubmitting.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: () => {
          if (navigateAfterSuccess) void this.router.navigateByUrl(this.safeReturnUrl());
        },
        error: () => this.submitFailed.set(true),
      });
  }

  private startResendCountdown(seconds: number): void {
    this.resendCountdown?.unsubscribe();
    const duration = Math.max(0, Math.floor(seconds));
    this.resendSeconds.set(duration);
    if (duration === 0) return;

    this.resendCountdown = interval(1000)
      .pipe(take(duration), takeUntilDestroyed(this.destroyRef))
      .subscribe((elapsed) => this.resendSeconds.set(duration - elapsed - 1));
  }

  private safeReturnUrl(): string {
    const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
    return returnUrl?.startsWith('/') && !returnUrl.startsWith('//')
      ? returnUrl
      : '/dashboard';
  }
}

import { Component, inject, input, output } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthenticationService } from '../../core/auth/authentication.service';
import {
  LocalizationService,
  TranslationKey,
} from '../../localization/localization.service';
import { LanguageSwitcher } from '../../shared/language-switcher/language-switcher';

interface NavigationItem {
  readonly label: TranslationKey;
  readonly route: string;
}

@Component({
  selector: 'app-sidebar',
  imports: [RouterLink, RouterLinkActive, LanguageSwitcher],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss',
})
export class Sidebar {
  private readonly router = inject(Router);
  protected readonly authentication = inject(AuthenticationService);
  protected readonly i18n = inject(LocalizationService);

  readonly isOpen = input(false);
  readonly closeRequested = output<void>();

  protected readonly navigation: readonly NavigationItem[] = [
    { label: 'dashboard', route: '/dashboard' },
  ];

  protected close(): void {
    this.closeRequested.emit();
  }

  protected logout(): void {
    this.authentication.logout();
    void this.router.navigate(['/login']);
  }
}

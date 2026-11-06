import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LocalizationService } from '../../localization/localization.service';
import { LanguageSwitcher } from '../../shared/language-switcher/language-switcher';

@Component({
  selector: 'app-not-found-page',
  imports: [RouterLink, LanguageSwitcher],
  templateUrl: './not-found-page.html',
  styleUrl: './not-found-page.scss',
})
export class NotFoundPage {
  protected readonly i18n = inject(LocalizationService);
}

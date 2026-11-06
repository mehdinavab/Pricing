import { Component, inject } from '@angular/core';
import { LocalizationService } from '../../localization/localization.service';

@Component({
  selector: 'app-language-switcher',
  template: `
    <div class="language-switcher" [attr.aria-label]="i18n.translate('languageSelector')">
      @for (option of i18n.languages; track option.code) {
        <button
          type="button"
          [class.active]="i18n.language() === option.code"
          [attr.aria-pressed]="i18n.language() === option.code"
          [attr.lang]="option.code"
          [attr.dir]="option.direction"
          (click)="i18n.setLanguage(option.code)"
        >
          {{ option.shortLabel }}
        </button>
      }
    </div>
  `,
  styles: `
    .language-switcher { display: flex; gap: .25rem; padding: .25rem; border: 1px solid var(--border); border-radius: .75rem; background: var(--surface-muted); }
    button { min-width: 2.25rem; min-height: 2rem; padding-inline: .55rem; border: 0; border-radius: .55rem; background: transparent; color: var(--text-muted); cursor: pointer; font: inherit; font-size: .75rem; font-weight: 700; }
    button:hover { color: var(--primary); background: var(--primary-soft); }
    button:focus-visible { outline: 3px solid var(--focus); outline-offset: 2px; }
    button.active { color: #fff; background: var(--primary); }
  `,
})
export class LanguageSwitcher {
  protected readonly i18n = inject(LocalizationService);
}

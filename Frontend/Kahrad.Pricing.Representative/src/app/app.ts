import { Component, inject } from '@angular/core';
import { LocalizationService } from './localization/localization.service';

@Component({
  selector: 'app-root',
  standalone: false,
  styleUrl: './app.scss',
  templateUrl: './app.html',
})
export class App {
  protected readonly i18n = inject(LocalizationService);
}

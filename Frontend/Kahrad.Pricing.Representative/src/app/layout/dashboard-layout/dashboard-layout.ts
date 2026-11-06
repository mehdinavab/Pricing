import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LocalizationService } from '../../localization/localization.service';
import { Sidebar } from '../sidebar/sidebar';

@Component({
  selector: 'app-dashboard-layout',
  imports: [RouterOutlet, Sidebar],
  templateUrl: './dashboard-layout.html',
  styleUrl: './dashboard-layout.scss',
})
export class DashboardLayout {
  protected readonly i18n = inject(LocalizationService);
  protected readonly menuOpen = signal(false);
}

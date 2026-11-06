import { Component, inject } from '@angular/core';
import { AuthenticationService } from '../../core/auth/authentication.service';
import {
  LocalizationService,
  TranslationKey,
} from '../../localization/localization.service';

interface DashboardMetric {
  readonly label: TranslationKey;
  readonly hint: TranslationKey;
  readonly value?: string;
  readonly valueKey?: TranslationKey;
  readonly tone: 'blue' | 'violet' | 'amber' | 'green';
}

@Component({
  selector: 'app-dashboard-page',
  templateUrl: './dashboard-page.html',
  styleUrl: './dashboard-page.scss',
})
export class DashboardPage {
  protected readonly authentication = inject(AuthenticationService);
  protected readonly i18n = inject(LocalizationService);
  protected readonly metrics: readonly DashboardMetric[] = [
    { label: 'metricProducts', hint: 'metricProductsHint', value: '35K', tone: 'blue' },
    {
      label: 'metricPricing',
      hint: 'metricPricingHint',
      valueKey: 'statusLive',
      tone: 'violet',
    },
    {
      label: 'metricRepresentatives',
      hint: 'metricRepresentativesHint',
      value: '3',
      tone: 'amber',
    },
    {
      label: 'metricStatus',
      hint: 'metricStatusHint',
      valueKey: 'statusActive',
      tone: 'green',
    },
  ];

  protected metricValue(metric: DashboardMetric): string {
    return metric.valueKey ? this.i18n.translate(metric.valueKey) : (metric.value ?? '');
  }
}

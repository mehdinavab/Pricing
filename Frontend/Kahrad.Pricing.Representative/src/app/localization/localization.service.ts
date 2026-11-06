import { DOCUMENT } from '@angular/common';
import { Injectable, computed, inject, signal } from '@angular/core';

export type SupportedLanguage = 'en' | 'fa' | 'ar';
export type TextDirection = 'ltr' | 'rtl';

const ENGLISH_TRANSLATIONS = {
  appTitle: 'Kahrad Representative Portal',
  eyebrow: 'Representative workspace',
  welcome: 'Welcome to your representative portal',
  description: 'Review products, current prices, and representative services in your preferred language.',
  languageSelector: 'Choose display language',
  english: 'English',
  persian: 'Persian',
  arabic: 'Arabic',
  statusTitle: 'Localization is ready',
  statusBody: 'Language preference and text direction are applied across the application.',
} as const;

export type TranslationKey = keyof typeof ENGLISH_TRANSLATIONS;
type TranslationDictionary = Record<TranslationKey, string>;

const TRANSLATIONS: Record<SupportedLanguage, TranslationDictionary> = {
  en: ENGLISH_TRANSLATIONS,
  fa: {
    appTitle: 'پرتال نمایندگان کهراد',
    eyebrow: 'فضای کاری نمایندگان',
    welcome: 'به پرتال نمایندگان خوش آمدید',
    description: 'محصولات، قیمت‌های فعلی و خدمات نمایندگان را به زبان دلخواه خود مشاهده کنید.',
    languageSelector: 'انتخاب زبان نمایش',
    english: 'انگلیسی',
    persian: 'فارسی',
    arabic: 'عربی',
    statusTitle: 'چندزبانه‌سازی آماده است',
    statusBody: 'زبان انتخاب‌شده و جهت متن در سراسر برنامه اعمال می‌شود.',
  },
  ar: {
    appTitle: 'بوابة ممثلي كهراد',
    eyebrow: 'مساحة عمل الممثلين',
    welcome: 'مرحباً بك في بوابة الممثلين',
    description: 'راجع المنتجات والأسعار الحالية وخدمات الممثلين بلغتك المفضلة.',
    languageSelector: 'اختر لغة العرض',
    english: 'الإنجليزية',
    persian: 'الفارسية',
    arabic: 'العربية',
    statusTitle: 'دعم اللغات جاهز',
    statusBody: 'يتم تطبيق اللغة المختارة واتجاه النص في جميع أجزاء التطبيق.',
  },
};

export interface LanguageOption {
  readonly code: SupportedLanguage;
  readonly direction: TextDirection;
  readonly labelKey: TranslationKey;
}

const LANGUAGE_OPTIONS: readonly LanguageOption[] = [
  { code: 'en', direction: 'ltr', labelKey: 'english' },
  { code: 'fa', direction: 'rtl', labelKey: 'persian' },
  { code: 'ar', direction: 'rtl', labelKey: 'arabic' },
];

@Injectable({ providedIn: 'root' })
export class LocalizationService {
  private readonly document = inject(DOCUMENT);
  private readonly storageKey = 'kahrad.pricing.representative.language';

  readonly languages = LANGUAGE_OPTIONS;
  readonly language = signal<SupportedLanguage>(this.resolveInitialLanguage());
  readonly direction = computed<TextDirection>(() =>
    this.language() === 'en' ? 'ltr' : 'rtl',
  );

  constructor() {
    this.applyDocumentLanguage(this.language());
  }

  setLanguage(language: SupportedLanguage): void {
    this.language.set(language);
    this.persistLanguage(language);
    this.applyDocumentLanguage(language);
  }

  translate(key: TranslationKey): string {
    return TRANSLATIONS[this.language()][key];
  }

  private resolveInitialLanguage(): SupportedLanguage {
    const storedLanguage = this.readStoredLanguage();
    if (storedLanguage) {
      return storedLanguage;
    }

    return this.normalizeLanguage(this.document.defaultView?.navigator.language) ?? 'en';
  }

  private readStoredLanguage(): SupportedLanguage | null {
    try {
      return this.normalizeLanguage(
        this.document.defaultView?.localStorage.getItem(this.storageKey),
      );
    } catch {
      return null;
    }
  }

  private persistLanguage(language: SupportedLanguage): void {
    try {
      this.document.defaultView?.localStorage.setItem(this.storageKey, language);
    } catch {
      // The selected language still applies when browser storage is unavailable.
    }
  }

  private normalizeLanguage(language: string | null | undefined): SupportedLanguage | null {
    const languageCode = language?.trim().toLowerCase().split('-')[0];
    return languageCode === 'en' || languageCode === 'fa' || languageCode === 'ar'
      ? languageCode
      : null;
  }

  private applyDocumentLanguage(language: SupportedLanguage): void {
    const root = this.document.documentElement;
    root.lang = language;
    root.dir = language === 'en' ? 'ltr' : 'rtl';
    this.document.title = TRANSLATIONS[language].appTitle;
  }
}

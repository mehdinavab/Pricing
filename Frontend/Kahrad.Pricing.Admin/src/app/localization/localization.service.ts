import { DOCUMENT } from '@angular/common';
import { Injectable, computed, inject, signal } from '@angular/core';

export type SupportedLanguage = 'en' | 'fa' | 'ar';
export type TextDirection = 'ltr' | 'rtl';

const ENGLISH_TRANSLATIONS = {
  appTitle: 'Kahrad Pricing Administration',
  eyebrow: 'Product pricing platform',
  welcome: 'Welcome to the administration workspace',
  description: 'Manage products, pricing policies, and operational settings from one place.',
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
    appTitle: 'مدیریت قیمت‌گذاری کهراد',
    eyebrow: 'سامانه قیمت‌گذاری محصولات',
    welcome: 'به فضای مدیریت خوش آمدید',
    description: 'محصولات، سیاست‌های قیمت‌گذاری و تنظیمات عملیاتی را از یک محل مدیریت کنید.',
    languageSelector: 'انتخاب زبان نمایش',
    english: 'انگلیسی',
    persian: 'فارسی',
    arabic: 'عربی',
    statusTitle: 'چندزبانه‌سازی آماده است',
    statusBody: 'زبان انتخاب‌شده و جهت متن در سراسر برنامه اعمال می‌شود.',
  },
  ar: {
    appTitle: 'إدارة تسعير كهراد',
    eyebrow: 'منصة تسعير المنتجات',
    welcome: 'مرحباً بك في مساحة الإدارة',
    description: 'أدر المنتجات وسياسات التسعير والإعدادات التشغيلية من مكان واحد.',
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
  private readonly storageKey = 'kahrad.pricing.admin.language';

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

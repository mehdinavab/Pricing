import { DOCUMENT } from '@angular/common';
import { Injectable, computed, inject, signal } from '@angular/core';

export type SupportedLanguage = 'en' | 'fa' | 'ar';
export type TextDirection = 'ltr' | 'rtl';

const ENGLISH_TRANSLATIONS = {
  appTitle: 'Kahrad Pricing Administration',
  appShortName: 'Kahrad Admin',
  languageSelector: 'Choose display language',
  loginEyebrow: 'Administration portal',
  loginTitle: 'Welcome back',
  loginDescription: 'Sign in to manage products, pricing, and representatives.',
  usernameLabel: 'Username',
  usernamePlaceholder: 'Enter your username',
  usernameRequired: 'Username is required.',
  passwordLabel: 'Password',
  passwordPlaceholder: 'Enter your password',
  passwordRequired: 'Password is required.',
  signIn: 'Sign in',
  signingIn: 'Signing in…',
  loginError: 'We could not sign you in. Check your details and try again.',
  demoNotice: 'Base UI only: connect this form to the approved identity API before production.',
  navigationLabel: 'Primary navigation',
  dashboard: 'Dashboard',
  logout: 'Sign out',
  openMenu: 'Open navigation',
  closeMenu: 'Close navigation',
  signedInAs: 'Signed in as',
  welcomeBack: 'Welcome back',
  dashboardTitle: 'Pricing operations at a glance',
  dashboardDescription: 'A focused overview of the catalog and pricing workspace.',
  dashboardMetricsLabel: 'Dashboard metrics',
  metricProducts: 'Products',
  metricProductsHint: 'Catalog records ready for management',
  metricPricing: 'Pricing policies',
  metricPricingHint: 'Brand and representative rules',
  metricRepresentatives: 'Representatives',
  metricRepresentativesHint: 'Profiles and commercial access',
  metricStatus: 'System status',
  metricStatusHint: 'Core services are available',
  statusOnline: 'Online',
  recentActivity: 'Recent activity',
  activityEmpty: 'Operational activity will appear here as modules are connected.',
  notFoundCode: '404',
  notFoundTitle: 'Page not found',
  notFoundDescription: 'The address may be incorrect or the page may have moved.',
  backToDashboard: 'Back to dashboard',
} as const;

export type TranslationKey = keyof typeof ENGLISH_TRANSLATIONS;
type TranslationDictionary = Record<TranslationKey, string>;

const TRANSLATIONS: Record<SupportedLanguage, TranslationDictionary> = {
  en: ENGLISH_TRANSLATIONS,
  fa: {
    appTitle: 'مدیریت قیمت‌گذاری کهراد',
    appShortName: 'مدیریت کهراد',
    languageSelector: 'انتخاب زبان نمایش',
    loginEyebrow: 'پرتال مدیریت',
    loginTitle: 'خوش آمدید',
    loginDescription: 'برای مدیریت محصولات، قیمت‌گذاری و نمایندگان وارد شوید.',
    usernameLabel: 'نام کاربری',
    usernamePlaceholder: 'نام کاربری را وارد کنید',
    usernameRequired: 'نام کاربری الزامی است.',
    passwordLabel: 'رمز عبور',
    passwordPlaceholder: 'رمز عبور را وارد کنید',
    passwordRequired: 'رمز عبور الزامی است.',
    signIn: 'ورود',
    signingIn: 'در حال ورود…',
    loginError: 'ورود انجام نشد. اطلاعات را بررسی و دوباره تلاش کنید.',
    demoNotice: 'نسخه پایه رابط کاربری است؛ پیش از انتشار به سامانه هویت مورد تأیید متصل شود.',
    navigationLabel: 'منوی اصلی',
    dashboard: 'داشبورد',
    logout: 'خروج',
    openMenu: 'باز کردن منو',
    closeMenu: 'بستن منو',
    signedInAs: 'کاربر فعال',
    welcomeBack: 'خوش آمدید',
    dashboardTitle: 'نمای کلی عملیات قیمت‌گذاری',
    dashboardDescription: 'مروری متمرکز بر فضای مدیریت کاتالوگ و قیمت‌گذاری.',
    dashboardMetricsLabel: 'شاخص‌های داشبورد',
    metricProducts: 'محصولات',
    metricProductsHint: 'رکوردهای آماده مدیریت در کاتالوگ',
    metricPricing: 'سیاست‌های قیمت‌گذاری',
    metricPricingHint: 'قوانین برند و نمایندگان',
    metricRepresentatives: 'نمایندگان',
    metricRepresentativesHint: 'پروفایل‌ها و دسترسی تجاری',
    metricStatus: 'وضعیت سامانه',
    metricStatusHint: 'سرویس‌های اصلی در دسترس هستند',
    statusOnline: 'برخط',
    recentActivity: 'فعالیت‌های اخیر',
    activityEmpty: 'پس از اتصال ماژول‌ها، فعالیت‌های عملیاتی در این بخش نمایش داده می‌شود.',
    notFoundCode: '۴۰۴',
    notFoundTitle: 'صفحه پیدا نشد',
    notFoundDescription: 'ممکن است نشانی اشتباه باشد یا صفحه جابه‌جا شده باشد.',
    backToDashboard: 'بازگشت به داشبورد',
  },
  ar: {
    appTitle: 'إدارة تسعير كهراد',
    appShortName: 'إدارة كهراد',
    languageSelector: 'اختر لغة العرض',
    loginEyebrow: 'بوابة الإدارة',
    loginTitle: 'مرحباً بعودتك',
    loginDescription: 'سجّل الدخول لإدارة المنتجات والتسعير والممثلين.',
    usernameLabel: 'اسم المستخدم',
    usernamePlaceholder: 'أدخل اسم المستخدم',
    usernameRequired: 'اسم المستخدم مطلوب.',
    passwordLabel: 'كلمة المرور',
    passwordPlaceholder: 'أدخل كلمة المرور',
    passwordRequired: 'كلمة المرور مطلوبة.',
    signIn: 'تسجيل الدخول',
    signingIn: 'جارٍ تسجيل الدخول…',
    loginError: 'تعذر تسجيل الدخول. تحقق من البيانات وحاول مرة أخرى.',
    demoNotice: 'هذه واجهة أساسية؛ اربط النموذج بخدمة الهوية المعتمدة قبل النشر.',
    navigationLabel: 'التنقل الرئيسي',
    dashboard: 'لوحة المعلومات',
    logout: 'تسجيل الخروج',
    openMenu: 'فتح القائمة',
    closeMenu: 'إغلاق القائمة',
    signedInAs: 'المستخدم الحالي',
    welcomeBack: 'مرحباً بعودتك',
    dashboardTitle: 'نظرة عامة على عمليات التسعير',
    dashboardDescription: 'ملخص مركز لمساحة إدارة الكتالوج والتسعير.',
    dashboardMetricsLabel: 'مؤشرات لوحة المعلومات',
    metricProducts: 'المنتجات',
    metricProductsHint: 'سجلات الكتالوج الجاهزة للإدارة',
    metricPricing: 'سياسات التسعير',
    metricPricingHint: 'قواعد العلامات التجارية والممثلين',
    metricRepresentatives: 'الممثلون',
    metricRepresentativesHint: 'الملفات والصلاحيات التجارية',
    metricStatus: 'حالة النظام',
    metricStatusHint: 'الخدمات الأساسية متاحة',
    statusOnline: 'متصل',
    recentActivity: 'النشاط الأخير',
    activityEmpty: 'سيظهر النشاط التشغيلي هنا بعد ربط الوحدات.',
    notFoundCode: '٤٠٤',
    notFoundTitle: 'الصفحة غير موجودة',
    notFoundDescription: 'قد يكون العنوان غير صحيح أو تم نقل الصفحة.',
    backToDashboard: 'العودة إلى لوحة المعلومات',
  },
};

export interface LanguageOption {
  readonly code: SupportedLanguage;
  readonly direction: TextDirection;
  readonly shortLabel: string;
}

const LANGUAGE_OPTIONS: readonly LanguageOption[] = [
  { code: 'en', direction: 'ltr', shortLabel: 'EN' },
  { code: 'fa', direction: 'rtl', shortLabel: 'فا' },
  { code: 'ar', direction: 'rtl', shortLabel: 'ع' },
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
    return (
      this.readStoredLanguage() ??
      this.normalizeLanguage(this.document.defaultView?.navigator.language) ??
      'en'
    );
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
      // Language still applies when preference storage is unavailable.
    }
  }

  private normalizeLanguage(language: string | null | undefined): SupportedLanguage | null {
    const code = language?.trim().toLowerCase().split('-')[0];
    return code === 'en' || code === 'fa' || code === 'ar' ? code : null;
  }

  private applyDocumentLanguage(language: SupportedLanguage): void {
    this.document.documentElement.lang = language;
    this.document.documentElement.dir = language === 'en' ? 'ltr' : 'rtl';
    this.document.title = TRANSLATIONS[language].appTitle;
  }
}

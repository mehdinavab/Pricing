import { DOCUMENT } from '@angular/common';
import { Injectable, computed, inject, signal } from '@angular/core';

export type SupportedLanguage = 'en' | 'fa' | 'ar';
export type TextDirection = 'ltr' | 'rtl';

const ENGLISH_TRANSLATIONS = {
  appTitle: 'Kahrad Representative Portal',
  appShortName: 'Kahrad Portal',
  languageSelector: 'Choose display language',
  loginEyebrow: 'Representative portal',
  loginTitle: 'Welcome back',
  loginDescription: 'Sign in to review products, prices, and representative services.',
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
  dashboardTitle: 'Your representative workspace',
  dashboardDescription: 'Review product and account information from one clear dashboard.',
  dashboardMetricsLabel: 'Dashboard metrics',
  metricProducts: 'Available products',
  metricProductsHint: 'Published products ready to review',
  metricPricing: 'Current prices',
  metricPricingHint: 'Prepared representative pricing',
  statusLive: 'Live',
  metricRepresentatives: 'Active offers',
  metricRepresentativesHint: 'Commercial opportunities for your account',
  metricStatus: 'Account status',
  metricStatusHint: 'Your representative profile is active',
  statusActive: 'Active',
  recentActivity: 'Recent activity',
  activityEmpty: 'Your latest pricing and account updates will appear here.',
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
    appTitle: 'پرتال نمایندگان کهراد',
    appShortName: 'پرتال کهراد',
    languageSelector: 'انتخاب زبان نمایش',
    loginEyebrow: 'پرتال نمایندگان',
    loginTitle: 'خوش آمدید',
    loginDescription: 'برای مشاهده محصولات، قیمت‌ها و خدمات نمایندگان وارد شوید.',
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
    dashboardTitle: 'فضای کاری نمایندگان',
    dashboardDescription: 'اطلاعات محصولات و حساب خود را در یک داشبورد شفاف مشاهده کنید.',
    dashboardMetricsLabel: 'شاخص‌های داشبورد',
    metricProducts: 'محصولات موجود',
    metricProductsHint: 'محصولات منتشرشده برای مشاهده',
    metricPricing: 'قیمت‌های فعلی',
    metricPricingHint: 'قیمت‌های آماده‌شده برای نماینده',
    statusLive: 'به‌روز',
    metricRepresentatives: 'پیشنهادهای فعال',
    metricRepresentativesHint: 'فرصت‌های تجاری ویژه حساب شما',
    metricStatus: 'وضعیت حساب',
    metricStatusHint: 'پروفایل نمایندگی شما فعال است',
    statusActive: 'فعال',
    recentActivity: 'فعالیت‌های اخیر',
    activityEmpty: 'آخرین تغییرات قیمت و حساب در این بخش نمایش داده می‌شود.',
    notFoundCode: '۴۰۴',
    notFoundTitle: 'صفحه پیدا نشد',
    notFoundDescription: 'ممکن است نشانی اشتباه باشد یا صفحه جابه‌جا شده باشد.',
    backToDashboard: 'بازگشت به داشبورد',
  },
  ar: {
    appTitle: 'بوابة ممثلي كهراد',
    appShortName: 'بوابة كهراد',
    languageSelector: 'اختر لغة العرض',
    loginEyebrow: 'بوابة الممثلين',
    loginTitle: 'مرحباً بعودتك',
    loginDescription: 'سجّل الدخول لمراجعة المنتجات والأسعار وخدمات الممثلين.',
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
    dashboardTitle: 'مساحة عمل الممثلين',
    dashboardDescription: 'راجع معلومات المنتجات والحساب من لوحة واضحة واحدة.',
    dashboardMetricsLabel: 'مؤشرات لوحة المعلومات',
    metricProducts: 'المنتجات المتاحة',
    metricProductsHint: 'المنتجات المنشورة الجاهزة للمراجعة',
    metricPricing: 'الأسعار الحالية',
    metricPricingHint: 'الأسعار المعدة للممثل',
    statusLive: 'مباشر',
    metricRepresentatives: 'العروض النشطة',
    metricRepresentativesHint: 'الفرص التجارية المتاحة لحسابك',
    metricStatus: 'حالة الحساب',
    metricStatusHint: 'ملف الممثل الخاص بك نشط',
    statusActive: 'نشط',
    recentActivity: 'النشاط الأخير',
    activityEmpty: 'ستظهر آخر تحديثات الأسعار والحساب هنا.',
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

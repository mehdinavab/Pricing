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
  signInMethod: 'Choose a sign-in method',
  passwordSignIn: 'Password',
  otpSignIn: 'SMS code',
  usernameLabel: 'Username',
  usernamePlaceholder: 'Enter your username',
  usernameRequired: 'Username is required.',
  passwordLabel: 'Password',
  passwordPlaceholder: 'Enter your password',
  passwordRequired: 'Password is required.',
  signIn: 'Sign in',
  signingIn: 'Signing in…',
  loginError: 'We could not sign you in. Check your details and try again.',
  otpRequestDescription: 'Enter the mobile number registered with your account.',
  phoneNumberLabel: 'Mobile number',
  phoneNumberPlaceholder: '0912 123 4567',
  phoneNumberInvalid: 'Enter a valid Iranian mobile number.',
  sendCode: 'Send verification code',
  sendingCode: 'Sending code…',
  otpRequestError: 'The code could not be requested. Please try again.',
  otpVerifyDescription: 'If the number matches an active account, a code has been sent.',
  otpCodeLabel: 'Verification code',
  otpCodePlaceholder: '6-digit code',
  otpCodeInvalid: 'Enter the verification code from the SMS.',
  verifyCode: 'Verify and sign in',
  verifyingCode: 'Verifying…',
  otpVerifyError: 'The code is invalid or expired. Request a new code and try again.',
  changePhoneNumber: 'Change number',
  resendIn: 'Resend in',
  resendCode: 'Resend code',
  demoNotice: 'Authentication uses the approved Identity API and keeps tokens for this browser session only.',
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
    signInMethod: 'روش ورود را انتخاب کنید',
    passwordSignIn: 'رمز عبور',
    otpSignIn: 'کد پیامکی',
    usernameLabel: 'نام کاربری',
    usernamePlaceholder: 'نام کاربری را وارد کنید',
    usernameRequired: 'نام کاربری الزامی است.',
    passwordLabel: 'رمز عبور',
    passwordPlaceholder: 'رمز عبور را وارد کنید',
    passwordRequired: 'رمز عبور الزامی است.',
    signIn: 'ورود',
    signingIn: 'در حال ورود…',
    loginError: 'ورود انجام نشد. اطلاعات را بررسی و دوباره تلاش کنید.',
    otpRequestDescription: 'شماره همراه ثبت‌شده در حساب خود را وارد کنید.',
    phoneNumberLabel: 'شماره همراه',
    phoneNumberPlaceholder: '۰۹۱۲ ۱۲۳ ۴۵۶۷',
    phoneNumberInvalid: 'یک شماره همراه معتبر ایران وارد کنید.',
    sendCode: 'ارسال کد تأیید',
    sendingCode: 'در حال ارسال کد…',
    otpRequestError: 'درخواست کد انجام نشد. دوباره تلاش کنید.',
    otpVerifyDescription: 'اگر شماره با یک حساب فعال مطابقت داشته باشد، کد ارسال شده است.',
    otpCodeLabel: 'کد تأیید',
    otpCodePlaceholder: 'کد ۶ رقمی',
    otpCodeInvalid: 'کد تأیید پیامک‌شده را وارد کنید.',
    verifyCode: 'تأیید و ورود',
    verifyingCode: 'در حال تأیید…',
    otpVerifyError: 'کد نامعتبر یا منقضی است. کد جدیدی درخواست کنید.',
    changePhoneNumber: 'تغییر شماره',
    resendIn: 'ارسال مجدد تا',
    resendCode: 'ارسال دوباره کد',
    demoNotice: 'احراز هویت از API هویت مورد تأیید استفاده می‌کند و توکن‌ها فقط در همین نشست مرورگر نگهداری می‌شوند.',
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
    signInMethod: 'اختر طريقة تسجيل الدخول',
    passwordSignIn: 'كلمة المرور',
    otpSignIn: 'رمز الرسالة',
    usernameLabel: 'اسم المستخدم',
    usernamePlaceholder: 'أدخل اسم المستخدم',
    usernameRequired: 'اسم المستخدم مطلوب.',
    passwordLabel: 'كلمة المرور',
    passwordPlaceholder: 'أدخل كلمة المرور',
    passwordRequired: 'كلمة المرور مطلوبة.',
    signIn: 'تسجيل الدخول',
    signingIn: 'جارٍ تسجيل الدخول…',
    loginError: 'تعذر تسجيل الدخول. تحقق من البيانات وحاول مرة أخرى.',
    otpRequestDescription: 'أدخل رقم الهاتف المحمول المسجل في حسابك.',
    phoneNumberLabel: 'رقم الهاتف المحمول',
    phoneNumberPlaceholder: '0912 123 4567',
    phoneNumberInvalid: 'أدخل رقم هاتف محمول إيرانياً صالحاً.',
    sendCode: 'إرسال رمز التحقق',
    sendingCode: 'جارٍ إرسال الرمز…',
    otpRequestError: 'تعذر طلب الرمز. حاول مرة أخرى.',
    otpVerifyDescription: 'إذا كان الرقم مرتبطاً بحساب نشط، فقد تم إرسال الرمز.',
    otpCodeLabel: 'رمز التحقق',
    otpCodePlaceholder: 'رمز من 6 أرقام',
    otpCodeInvalid: 'أدخل رمز التحقق الموجود في الرسالة.',
    verifyCode: 'التحقق وتسجيل الدخول',
    verifyingCode: 'جارٍ التحقق…',
    otpVerifyError: 'الرمز غير صالح أو منتهي الصلاحية. اطلب رمزاً جديداً.',
    changePhoneNumber: 'تغيير الرقم',
    resendIn: 'إعادة الإرسال خلال',
    resendCode: 'إعادة إرسال الرمز',
    demoNotice: 'تستخدم المصادقة واجهة الهوية المعتمدة، وتُحفظ الرموز لهذه الجلسة فقط.',
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

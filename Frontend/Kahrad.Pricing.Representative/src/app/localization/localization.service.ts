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

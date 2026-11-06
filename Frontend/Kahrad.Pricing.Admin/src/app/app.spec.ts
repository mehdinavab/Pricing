import { TestBed } from '@angular/core/testing';
import { App } from './app';
import { AppRoutingModule } from './app-routing-module';
import { LocalizationService } from './localization/localization.service';

describe('App', () => {
  beforeEach(async () => {
    localStorage.clear();
    document.documentElement.lang = 'en';
    document.documentElement.dir = 'ltr';

    await TestBed.configureTestingModule({
      imports: [AppRoutingModule],
      declarations: [App],
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should render the English administration title', async () => {
    const fixture = TestBed.createComponent(App);
    await fixture.whenStable();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('h1')?.textContent).toContain(
      'Kahrad Pricing Administration',
    );
  });

  it('should switch to Persian and apply RTL document metadata', () => {
    const fixture = TestBed.createComponent(App);
    const localization = TestBed.inject(LocalizationService);

    localization.setLanguage('fa');
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('h1')?.textContent).toContain('مدیریت قیمت‌گذاری کهراد');
    expect(document.documentElement.lang).toBe('fa');
    expect(document.documentElement.dir).toBe('rtl');
    expect(localStorage.getItem('kahrad.pricing.admin.language')).toBe('fa');
  });
});

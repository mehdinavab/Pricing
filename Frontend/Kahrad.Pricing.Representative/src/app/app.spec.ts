import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { App } from './app';
import { LocalizationService } from './localization/localization.service';

describe('App', () => {
  beforeEach(async () => {
    localStorage.clear();
    document.documentElement.lang = 'en';
    document.documentElement.dir = 'ltr';

    await TestBed.configureTestingModule({
      imports: [App],
      providers: [provideRouter([])],
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should apply the English representative title', () => {
    TestBed.inject(LocalizationService);
    expect(document.title).toBe('Kahrad Representative Portal');
  });

  it('should switch to Arabic and apply RTL document metadata', () => {
    const localization = TestBed.inject(LocalizationService);

    localization.setLanguage('ar');

    expect(document.title).toBe('بوابة ممثلي كهراد');
    expect(document.documentElement.lang).toBe('ar');
    expect(document.documentElement.dir).toBe('rtl');
    expect(localStorage.getItem('kahrad.pricing.representative.language')).toBe('ar');
  });
});

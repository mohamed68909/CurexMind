import { create } from 'zustand';
import { translations, type Language } from '../i18n/translations';

interface SettingsState {
  theme: 'light' | 'dark';
  language: Language;
  toggleTheme: () => void;
  setLanguage: (lang: Language) => void;
  initializeSettings: () => void;
  t: (keyPath: string) => string;
}

const getInitialTheme = (): 'light' | 'dark' => {
  if (typeof window === 'undefined') return 'light';
  const storedTheme = localStorage.getItem('theme');
  if (storedTheme === 'light' || storedTheme === 'dark') {
    return storedTheme;
  }
  const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
  return prefersDark ? 'dark' : 'light';
};

const getInitialLanguage = (): Language => {
  if (typeof window === 'undefined') return 'ar';
  const storedLang = localStorage.getItem('language');
  if (storedLang === 'ar' || storedLang === 'en') {
    return storedLang;
  }
  return 'ar'; // Default language is Arabic
};

export const useSettingsStore = create<SettingsState>((set, get) => ({
  theme: getInitialTheme(),
  language: getInitialLanguage(),

  toggleTheme: () => {
    const nextTheme = get().theme === 'light' ? 'dark' : 'light';
    localStorage.setItem('theme', nextTheme);
    if (nextTheme === 'dark') {
      document.documentElement.classList.add('dark');
    } else {
      document.documentElement.classList.remove('dark');
    }
    set({ theme: nextTheme });
  },

  setLanguage: (lang: Language) => {
    localStorage.setItem('language', lang);
    const dir = lang === 'ar' ? 'rtl' : 'ltr';
    document.documentElement.setAttribute('dir', dir);
    document.documentElement.setAttribute('lang', lang);
    set({ language: lang });
  },

  initializeSettings: () => {
    const currentTheme = get().theme;
    if (currentTheme === 'dark') {
      document.documentElement.classList.add('dark');
    } else {
      document.documentElement.classList.remove('dark');
    }

    const currentLang = get().language;
    const dir = currentLang === 'ar' ? 'rtl' : 'ltr';
    document.documentElement.setAttribute('dir', dir);
    document.documentElement.setAttribute('lang', currentLang);
  },

  t: (keyPath: string) => {
    const [category, key] = keyPath.split('.');
    const lang = get().language;
    
    // Safety check
    const dict = translations[lang] as any;
    if (dict && dict[category] && dict[category][key] !== undefined) {
      return dict[category][key];
    }
    
    // Fallback to English
    const enDict = translations.en as any;
    if (enDict && enDict[category] && enDict[category][key] !== undefined) {
      return enDict[category][key];
    }

    return keyPath;
  }
}));

import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import {
  Auth,
  signInWithPopup,
  signOut,
  GoogleAuthProvider,
  OAuthProvider,
  FacebookAuthProvider,
} from '@angular/fire/auth';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface UserProfile {
  id: number;
  email: string;
  firstName: string | null;
  lastName: string | null;
  pictureUrl: string | null;
  provider: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly auth = inject(Auth);
  private readonly router = inject(Router);

  readonly currentUser = signal<UserProfile | null>(null);
  readonly isLoading = signal(false);

  async loginWithGoogle(): Promise<void> {
    await this.signInWithProvider(new GoogleAuthProvider());
  }

  async loginWithApple(): Promise<void> {
    const provider = new OAuthProvider('apple.com');
    provider.addScope('name');
    provider.addScope('email');
    await this.signInWithProvider(provider);
  }

  async loginWithFacebook(): Promise<void> {
    await this.signInWithProvider(new FacebookAuthProvider());
  }

  async logout(): Promise<void> {
    try {
      await firstValueFrom(
        this.http.post(`${environment.apiUrl}/auth/logout`, {}, { withCredentials: true })
      );
    } finally {
      await signOut(this.auth);
      this.currentUser.set(null);
      this.router.navigate(['/']);
    }
  }

  async checkSession(): Promise<boolean> {
    try {
      const user = await firstValueFrom(
        this.http.get<UserProfile>(`${environment.apiUrl}/auth/me`, { withCredentials: true })
      );
      this.currentUser.set(user);
      return true;
    } catch {
      this.currentUser.set(null);
      return false;
    }
  }

  private async signInWithProvider(provider: GoogleAuthProvider | OAuthProvider | FacebookAuthProvider): Promise<void> {
    this.isLoading.set(true);
    try {
      const result = await signInWithPopup(this.auth, provider);
      const firebaseToken = await result.user.getIdToken();
      const recaptchaToken = await this.getRecaptchaToken('login');

      const user = await firstValueFrom(
        this.http.post<UserProfile>(
          `${environment.apiUrl}/auth/login`,
          { firebaseToken, recaptchaToken },
          { withCredentials: true }
        )
      );
      this.currentUser.set(user);
      this.router.navigate(['/app']);
    } finally {
      this.isLoading.set(false);
    }
  }

  private getRecaptchaToken(action: string): Promise<string> {
    return new Promise((resolve) => {
      const siteKey = environment.recaptchaSiteKey;
      if (!siteKey || !(window as any).grecaptcha) {
        resolve('');
        return;
      }
      (window as any).grecaptcha.ready(() => {
        (window as any).grecaptcha.execute(siteKey, { action }).then(resolve);
      });
    });
  }
}

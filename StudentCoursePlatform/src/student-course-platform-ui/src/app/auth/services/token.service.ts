import { Injectable, inject, PLATFORM_ID } from "@angular/core";
import { isPlatformBrowser } from "@angular/common";
import { LoginResponseDto, UserResponseDto } from "../../shared/models/user/user.response";

@Injectable({
  providedIn: 'root'
})
export class TokenService {
  private readonly ACCESS_KEY = 'access_token';
  private readonly REFRESH_KEY = 'refresh_token';
  private readonly EXPIRE_KEY = 'access_token_expire';
  private readonly USER_KEY = 'current_user';

  private platformId = inject(PLATFORM_ID);

  private isBrowser(): boolean {
    return isPlatformBrowser(this.platformId);
  }

  setToken(res: LoginResponseDto): void {
    if (!this.isBrowser()) return;

    localStorage.setItem(this.ACCESS_KEY, res.accessToken);
    localStorage.setItem(this.REFRESH_KEY, res.refreshToken);
    localStorage.setItem(this.EXPIRE_KEY, res.accessTokenExpireAt);
    localStorage.setItem(this.USER_KEY, JSON.stringify(res.user));
  }

  getAccessToken(): string | null {
    if (!this.isBrowser()) return null;
    return localStorage.getItem(this.ACCESS_KEY);
  }

  getRefreshToken(): string | null {
    if (!this.isBrowser()) return null;
    return localStorage.getItem(this.REFRESH_KEY);
  }

  getCurrentUser(): UserResponseDto | null {
    if (!this.isBrowser()) return null;

    const raw = localStorage.getItem(this.USER_KEY);
    if (!raw) return null;

    try {
      return JSON.parse(raw);
    } catch {
      this.clear();
      return null;
    }
  }

  isLoggedIn(): boolean {
    if (!this.isBrowser()) return false;

    const token = this.getAccessToken();
    const expire = localStorage.getItem(this.EXPIRE_KEY);

    if (!token || !expire) return false;

    return new Date(expire) > new Date();
  }

  clear(): void {
    if (!this.isBrowser()) return;

    localStorage.removeItem(this.ACCESS_KEY);
    localStorage.removeItem(this.REFRESH_KEY);
    localStorage.removeItem(this.EXPIRE_KEY);
    localStorage.removeItem(this.USER_KEY);
  }
}
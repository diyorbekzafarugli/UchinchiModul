import { HttpClient } from "@angular/common/http";
import { inject, Injectable, signal } from "@angular/core";
import { TokenService } from "./token.service";
import { Router } from "@angular/router";
import { LoginResponseDto, UserResponseDto } from "../../shared/models/user/user.response";
import { UserLoginDto } from "../../shared/models/user/user.requests";
import { Observable, tap } from "rxjs";

@Injectable({ providedIn: 'root'})
export class AuthService {
    private http = inject(HttpClient);
    private token = inject(TokenService);
    private router = inject(Router);

    currentUser = signal<UserResponseDto | null>(this.token.getCurrentUser());

    login(dto: UserLoginDto): Observable<LoginResponseDto> {
        return this.http.post<LoginResponseDto>('/auth/login', dto)
        .pipe(
            tap(res => {
                this.token.setToken(res);
                this.currentUser.set(res.user);
            })
        );
    }

    logout(): void {
        this.token.clear();
        this.currentUser.set(null);
        this.router.navigate(['/auth/login']);
    }

    isLoggedIn() : boolean {
        return this.token.isLoggedIn();
    }

    getRole() : number | null {
        return this.currentUser()?.userRole ?? null;
    }
}
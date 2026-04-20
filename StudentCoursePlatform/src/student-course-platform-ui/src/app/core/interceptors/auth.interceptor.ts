import { HttpErrorResponse, HttpInterceptorFn } from "@angular/common/http";
import { TokenService } from "../../auth/services/token.service";
import { inject } from "@angular/core";
import { Router } from "@angular/router";
import { catchError, throwError } from "rxjs";

export const authInterceptor: HttpInterceptorFn = (req, next) => {
    const token = inject(TokenService);
    const router = inject(Router);

    const accessToken = token.getAccessToken();

    const authReq = accessToken
        ? req.clone({setHeaders: { Authorization: `Bearer ${accessToken}`}}) : req;

    return next(authReq).pipe(
        catchError((err: HttpErrorResponse) => {
            if(err.status === 401) {
                token.clear();
                router.navigate(['/auth/login']);
            }
            return throwError(() => err);
        })
    )
}
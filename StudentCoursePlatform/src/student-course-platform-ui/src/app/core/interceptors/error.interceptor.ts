import { HttpErrorResponse, HttpInterceptorFn } from "@angular/common/http";
import { inject } from "@angular/core";
import { TokenService } from "../../auth/services/token.service";
import { Router } from "@angular/router";
import { catchError, throwError } from "rxjs";


export const errorInterceptor: HttpInterceptorFn = (req, next) => {
    const token = inject(TokenService);
    const router = inject(Router);

    return next(req).pipe(
        catchError((err: HttpErrorResponse) => {
            switch(err.status){
                case 401:
                    token.clear();
                    router.navigate(['/auth/login']);
                    break;
                case 403:
                    router.navigate(['/forbidden']);
                    break;
                case 404:
                    console.error('Underfaind: ', err.message);
                    break;
                case 500:
                    console.error('Server error: ', err.message);
                    break;
            }

            return throwError(() => err);
        })
    )
}
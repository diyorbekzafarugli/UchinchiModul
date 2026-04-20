import { inject } from "@angular/core"
import { TokenService } from "../services/token.service"
import { Router } from "@angular/router";


export const guestGuard = () => {
    const tokenService = inject(TokenService);
    const router = inject(Router);

    if(!tokenService.isLoggedIn()) return true;

    return router.createUrlTree(['/dashboard']);
}
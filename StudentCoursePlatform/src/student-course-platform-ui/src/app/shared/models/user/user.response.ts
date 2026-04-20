import { UserRole } from "./user.model";


export interface UserResponseDto {
    id: string;
    fullName: string;
    email: string;
    userRole: UserRole;
    createdAt: string;
}

export interface LoginResponseDto {
    accessToken: string;
    refreshToken: string;
    accessTokenExpireAt: string;
    user: UserResponseDto;
}
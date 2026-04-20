import { UserRole } from "./user.model";

export interface UserRegisterDto {
    fullName: string;
    email: string;
    password: string;
}

export interface UserLoginDto {
    email: string;
    password: string;
}

export interface UserUpdateDto {
    fullName: string;
}

export interface UserCreateDto {
    fullName: string;
    email: string;
    password: string;
    userRole: UserRole;
}

export interface ChangePasswordDto {
    currentPassword: string;
    newPassword: string;
}
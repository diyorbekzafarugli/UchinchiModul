export interface UserModel {
    id: string;
    fullName: string;
    email: string;
    userRole: UserRole;
    createdAt: string;
}

export enum UserRole
{
    Student = 1,
    Teacher = 2,
    Admin = 3
}

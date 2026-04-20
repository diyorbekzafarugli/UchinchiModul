export interface CourseModel {
    id: string;
    title: string;
    description: string;
    imgUrl?: string;
    teacherId: string;
    teacherName: string;
    isPublished: boolean;
    createdAt: string;
    updatedAt?: string;
}

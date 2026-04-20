
export interface GetCourseDto {
    id: string;
    title: string;
    description: string;
    imgUrl?: string;
    teacherId: string;
    teacherName: string;
    isPublished: boolean;
    createdAt: string;
    updatedAt?: string;
    lessonCount: number;
    enrollmentCount: number;
}

export interface CourseSummaryDto {
    id: string;
    title: string;
    imgUrl?: string;
    teacherName: string;
    isPublished: boolean;
    lessonCount: number;
}
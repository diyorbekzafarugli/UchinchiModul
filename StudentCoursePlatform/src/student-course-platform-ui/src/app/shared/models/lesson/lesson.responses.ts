
export interface GetLessonDto {
    id: string;
    courseId: string;
    title: string;
    content?: string;
    order: number;
    videoUrl?: string;
    fileUrl?: string;
    isPublished: boolean;
    createdAt: string;
    updatedAt?: string;
}

export interface LessonSummaryDto {
    id: string;
    title: string;
    order: number;
    isPublished: boolean;
}
export interface LessonModel {
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

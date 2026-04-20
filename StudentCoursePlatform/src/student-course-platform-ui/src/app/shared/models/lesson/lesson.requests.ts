
export interface CreateLessonDto {
    title: string;
    content?: string;
    order: number;
    videoUrl?: string;
    fileUrl?: string;
    isPublished: boolean;
}

export interface UpdateLessonDto {
    title?: string;
    content?: string;
    order?: number;
    videoUrl?: string;
    fileUrl?: string;
    isPublished?: boolean;
}
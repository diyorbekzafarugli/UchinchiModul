
export interface CreateCourseDto {
    title: string;
    description: string;
    imgUrl?: string;
    teacherId: string;
}

export interface UpdateCourseDto {
    title?: string;
    description?: string;
    imgUrl?: string;
}
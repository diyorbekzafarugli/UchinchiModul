import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Course {
  id: number;
  title: string;
  mentor: string;
  duration: string;
  price: string;
}

@Injectable({
  providedIn: 'root'
})
export class CourseService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7222/api/courses'; 

  getCourses(pageIndex: number = 1, pageSize: number = 10, searchTerm: string = ''): Observable<Course[]> {
    let params = new HttpParams()
      .set('PageIndex', pageIndex.toString())
      .set('PageSize', pageSize.toString());

    if (searchTerm) {
      params = params.set('searchTerm', searchTerm);
    }

    return this.http.get<Course[]>(this.apiUrl, { params });
  }
}
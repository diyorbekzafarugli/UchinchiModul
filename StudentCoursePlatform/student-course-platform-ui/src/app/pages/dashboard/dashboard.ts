import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CourseService, Course } from '../../services/course.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class DashboardComponent implements OnInit {
  private courseService = inject(CourseService);
  
  // Bo'sh ro'yxat yaratamiz
  courses: Course[] = [];

  ngOnInit() {
    // Sahifa yuklanishi bilan kurslarni olib kelamiz
    this.loadCourses();
  }

    loadCourses() {
    // 1-sahifa, 10 ta kurs ko'rinishida so'raymiz
    this.courseService.getCourses(1, 10).subscribe({
        next: (data) => {
        this.courses = data;
        console.log("Bazadan kelgan kurslar:", data);
        },
        error: (err) => {
        console.error("Xatolik tafsiloti:", err);
        }
    });
    }
}
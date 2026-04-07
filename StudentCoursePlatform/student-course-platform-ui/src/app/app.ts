import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet], // <-- Shu so'z aniq bo'lishi shart!
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class AppComponent {
  title = 'student-course-platform-ui';
}
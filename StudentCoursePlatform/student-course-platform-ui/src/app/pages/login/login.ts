import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class LoginComponent {
  http = inject(HttpClient);
  router = inject(Router); 

  loginData = {
    email: '',
    password: ''
  };

  onSubmit() {
    const apiUrl = 'https://localhost:7222/api/auth/login'; 

    console.log("Kirishga urinish:", this.loginData);

    this.http.post(apiUrl, this.loginData).subscribe({
      next: (data: any) => {
        alert("Tizimga muvaffaqiyatli kirdingiz!");
        
        if (data && data.token) {
          localStorage.setItem('token', data.token);
        }

        this.router.navigate(['/dashboard']); 
      },
      error: (xato) => {
        console.error("Xatolik yuz berdi:", xato);
        alert("Email yoki parol noto'g'ri!");
      }
    });
  }
}
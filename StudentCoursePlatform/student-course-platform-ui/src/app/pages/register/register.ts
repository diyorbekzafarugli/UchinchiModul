import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.scss'
})
export class RegisterComponent {
  http = inject(HttpClient);
  router = inject(Router);

  registerData = {
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    confirmPassword: ''
  };

  onSubmit() {
    if (this.registerData.password !== this.registerData.confirmPassword) {
      alert("Parollar mos tushmadi! Iltimos, qayta tekshiring.");
      return;
    }

    const combinedFullName = this.registerData.firstName.trim() + ' ' + this.registerData.lastName.trim();

    const dataToSend = {
      fullName: combinedFullName,
      email: this.registerData.email,
      password: this.registerData.password
    };

    const apiUrl = 'https://localhost:7222/api/auth/register'; 

    this.http.post(apiUrl, dataToSend).subscribe({
      next: (javob) => {
        alert("Muvaffaqiyatli ro'yxatdan o'tdingiz!");
        this.router.navigate(['/login']);
      },
      error: (err) => {
        console.error("Xatolik yuz berdi:", err);
        alert("Xatolik! Bu email avval ishlatilgan bo'lishi mumkin.");
      }
    });
  }
}
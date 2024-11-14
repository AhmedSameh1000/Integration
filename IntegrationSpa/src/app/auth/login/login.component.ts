import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/Services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent {
  constructor(private AuthService: AuthService, private Router: Router) {}
  ngOnInit(): void {
    this.InializeLogInForm();
  }
  LogInForm: FormGroup;

  InializeLogInForm() {
    this.LogInForm = new FormGroup({
      Email: new FormControl(null, [Validators.required]),
      Password: new FormControl(null, Validators.required),
      RememberMe: new FormControl(false),
    });
  }
  PasswordOrEmailisWront: any;
  LogIn() {
    this.AuthService.LogIn(this.LogInForm.value).subscribe({
      next: (res: any) => {
        if (this.LogInForm.get('RememberMe').value == true) {
          localStorage.setItem('token', res.token);
        } else {
          sessionStorage.setItem('token', res.token);
        }
        this.Router.navigate(['/databases']);
      },
      error: (err) => {
        this.PasswordOrEmailisWront = err.error.message;
      },
    });
  }
}

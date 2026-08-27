import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';

import {
  MatCard,
  MatCardActions,
  MatCardContent,
  MatCardHeader,
  MatCardSubtitle,
  MatCardTitle,
} from '@angular/material/card';

import {
  MatError,
  MatFormField,
  MatLabel
} from '@angular/material/form-field';

import { MatInput } from '@angular/material/input';
import { MatButton } from '@angular/material/button';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
  imports: [
    ReactiveFormsModule,
    RouterLink,

    MatCard,
    MatCardHeader,
    MatCardTitle,
    MatCardSubtitle,
    MatCardContent,
    MatCardActions,

    MatFormField,
    MatLabel,
    MatError,
    MatInput,
    MatButton,
  ],
})
export class LoginComponent {
  loginForm = new FormGroup({
    email: new FormControl('', {
      validators: [Validators.required, Validators.email],
      nonNullable: true,
    }),
  });

  errorMessage = '';
  notFound = false;
  submitting = false;

  constructor(
    private router: Router,
    private authService: AuthService,
  ) {}

  requestCode(): void {
    if (this.loginForm.invalid || this.submitting) return;

    this.errorMessage = '';
    this.notFound = false;
    const email = this.loginForm.controls.email.value;

    this.submitting = true;

    this.authService.requestCode(email).subscribe({
      next: () => {
        this.router.navigate(['/verify'], { queryParams: { email } });
      },

      error: (err) => {
        this.submitting = false;

        if (err.status === 404) {
          this.notFound = true;

          this.errorMessage =
            'No account found for this email. Please register first.';
        } else {
          this.errorMessage =
            'Something went wrong. Please try again shortly.';
        }
      },
    });
  }
}

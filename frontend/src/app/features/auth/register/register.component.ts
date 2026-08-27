import { Component } from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { mergeMap } from 'rxjs';
import { AuthService } from '../../../core/auth/auth.service';

import {
  MatCard,
  MatCardActions,
  MatCardContent,
  MatCardHeader,
  MatCardSubtitle,
  MatCardTitle
} from '@angular/material/card';
import {
  MatError,
  MatFormField,
  MatHint,
  MatLabel
} from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatButton } from '@angular/material/button';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
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
    MatHint,
    MatInput,
    MatButton
  ]
})
export class RegisterComponent {
  registerForm = new FormGroup({
    firstName: new FormControl('', {
      validators: [Validators.required],
      nonNullable: true
    }),
    lastName: new FormControl('', {
      validators: [Validators.required],
      nonNullable: true
    }),
    email: new FormControl('', {
      validators: [Validators.required, Validators.email],
      nonNullable: true
    }),
    phone: new FormControl<string | null>(null)
  });

  errorMessage = '';
  submitting = false;

  constructor(
    private router: Router,
    private authService: AuthService
  ) {}

  register(): void {
    if (this.registerForm.invalid || this.submitting) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.errorMessage = '';
    this.submitting = true;

    const email = this.registerForm.controls.email.value;

    this.authService
      .register(this.registerForm.getRawValue())
      .pipe(mergeMap(() => this.authService.requestCode(email)))
      .subscribe({
        next: () => {
          this.router.navigate(['/verify'], { queryParams: { email } });
        },
        error: (err) => {
          this.submitting = false;
          if (err.status === 409) {
            this.errorMessage = 'Email already registered.';
          } else if (err.status === 400) {
            this.errorMessage = 'Please check the entered information.';
          } else {
            this.errorMessage = 'Registration failed. Please try again.';
          }
        }
      });
  }
}

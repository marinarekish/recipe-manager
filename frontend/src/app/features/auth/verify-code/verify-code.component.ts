import { Component } from '@angular/core';
import {FormControl, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
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
  MatLabel,
} from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatButton } from '@angular/material/button';

@Component({
  selector: 'app-verify-code',
  templateUrl: './verify-code.component.html',
  styleUrl: './verify-code.component.scss',
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
    FormsModule,
  ],
})
export class VerifyCodeComponent {
  email: string;

  code = new FormControl('', {
    validators: [
      Validators.required,
      Validators.minLength(6),
      Validators.maxLength(6),
      Validators.pattern(/^\d{6}$/),
    ],
    nonNullable: true,
  });

  errorMessage = '';
  notFound = false;
  resendMessage = '';
  submitting = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService,
  ) {
    const email = this.route.snapshot.queryParamMap.get('email');

    if (!email) {
      this.router.navigate(['/login']);
    }

    this.email = email ?? '';
  }

  verify(): void {
    if (this.code.invalid || this.submitting) return;

    this.errorMessage = '';
    this.notFound = false;
    this.submitting = true;

    this.authService.verifyCode(this.email, this.code.value).subscribe({
      next: () => {
        if (this.authService.getToken()) {
          this.router.navigate(['/recipes']);
        } else {
          this.submitting = false;
          this.errorMessage =
            'Your session could not be created. Please try again.';
        }
      },
      error: (err) => {
        this.submitting = false;
        if (err.status === 401) {
          this.errorMessage = 'The code is invalid or has expired.';
        } else if (err.status === 404) {
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

  resend(): void {
    if (this.submitting) return;

    this.errorMessage = '';
    this.notFound = false;
    this.resendMessage = '';

    this.submitting = true;

    this.authService.requestCode(this.email).subscribe({
      next: () => {
        this.submitting = false;
        this.resendMessage = 'A new code has been sent. Check the API logs.';
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

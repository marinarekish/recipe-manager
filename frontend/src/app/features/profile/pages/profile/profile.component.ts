import { Component, inject, OnInit } from '@angular/core';

import { UserService } from '../../data/user.service';
import { AuthService } from '../../../../core/auth/auth.service';
import { UserDto } from '../../../../core/auth/auth.models';
import { UpdateUserRequest } from '../../data/user.models';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import {Router} from '@angular/router';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss',
})

export class ProfileComponent implements OnInit {

  private userService = inject(UserService);
  private authService = inject(AuthService);
  private router = inject(Router);

  private formBuilder = inject(FormBuilder);

  user: UserDto | null = null;
  submitting = false;
  errorMessage = '';

  userForm = this.formBuilder.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    phone: [''],
  });

  get isAdmin(): boolean {
    return this.user?.roles?.some(r => r.name === 'Administrator') ?? false;
  }

  get initials(): string {
    const u = this.user;
    if (!u) {
      return '';
    }
    const source = u.firstName || u.email || '';
    return source.trim().charAt(0).toUpperCase();
  }

  ngOnInit(): void {
    this.load();
  }

  private load() {
    this.errorMessage = '';
    this.user = null;

    this.userService.getMe().subscribe({
      next: (user) => {
        this.user = user;

        this.userForm.patchValue({
          firstName: user.firstName,
          lastName: user.lastName,
          phone: user.phone,
        });
      },
      error: (err) => {
        this.user = null;
        this.errorMessage = 'Unable to load user';
      }
    })
  }

  submit() {
    if (this.userForm.invalid) {
      this.userForm.markAllAsTouched();
      return;
    }

    const v = this.userForm.getRawValue();
    const body: UpdateUserRequest = {
      firstName: v.firstName!.trim(),
      lastName: v.lastName!.trim(),
      phone: v.phone?.trim() ? v.phone.trim() : null,
    };

    this.submitting = true;

    this.updateUser(body);
  }

  private updateUser(body: UpdateUserRequest): void {
    this.userService.updateMe(body).subscribe({
      next: (user) => {
        this.submitting = false;
        this.errorMessage = '';

        this.user = user;
        this.userForm.patchValue({
          firstName: user.firstName,
          lastName: user.lastName,
          phone: user.phone,
        });

        this.authService.updateCurrentUser(user);

      },
      error: (err) => {
        this.submitting = false;
        this.errorMessage = 'Something went wrong';
      }
    })
  }

  deleteAccount() {
    if (!this.user) {
      return;
    }

    if (!window.confirm('Are you sure you want to delete your account? This cannot be undone.')) {
      return;
    }

    this.submitting = true;
    this.deleteUser(this.user);
  }

  private deleteUser(user: UserDto) {
    this.userService.delete(user.userId).subscribe({
      next: () => {
        this.submitting = false;

        this.authService.logout();
        void this.router.navigateByUrl('/login')
      },
      error: (err) => {
        this.submitting = false;
        this.errorMessage = 'Cannot delete user';
      }
    })
  }
}

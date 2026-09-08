import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { UserDto } from '../../../../core/auth/auth.models';
import { AdminUserService } from '../../data/admin-user.service';

const ROLE_ADMIN = 1;
const ROLE_USER = 2;

@Component({
  selector: 'app-admin-users',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './admin-users.component.html',
  styleUrl: './admin-users.component.scss',
})
export class AdminUsersComponent implements OnInit {
  readonly ROLE_ADMIN = ROLE_ADMIN;
  readonly ROLE_USER = ROLE_USER;

  private readonly adminUserService = inject(AdminUserService);

  users: UserDto[] = [];
  loading = true;
  errorMessage = '';
  busyUsers = new Set<number>();

  ngOnInit(): void {
    this.load();
  }

  load(preserveError = false): void {
    this.loading = true;
    if (!preserveError) {
      this.errorMessage = '';
    }

    this.adminUserService.getUsers().subscribe({
      next: (users) => {
        this.users = users ?? [];
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        if (!preserveError) {
          this.errorMessage = 'Unable to load users.';
        }
      },
    });
  }

  roleIdOf(user: UserDto): number {
    return this.isAdmin(user) ? ROLE_ADMIN : ROLE_USER;
  }

  isAdmin(user: UserDto): boolean {
    return user.roles?.some((r) => r.name === 'Administrator') ?? false;
  }

  initials(user: UserDto): string {
    const ch = user.firstName?.[0] ?? user.email?.[0];
    return ch?.toUpperCase() ?? '?';
  }

  onRoleChange(user: UserDto, roleId: number): void {
    if (roleId === this.roleIdOf(user)) {
      return;
    }

    this.busyUsers.add(user.userId);
    this.adminUserService.setUserRole(user.userId, roleId).subscribe({
      next: (updated) => {
        this.busyUsers.delete(user.userId);
        this.replaceUser(updated);
      },
      error: (err) => {
        this.busyUsers.delete(user.userId);
        this.errorMessage = this.roleError(err, roleId);
        this.load(true);
      },
    });
  }

  delete(user: UserDto): void {
    if (!window.confirm(`Delete ${user.firstName} ${user.lastName}? This cannot be undone.`)) {
      return;
    }

    this.busyUsers.add(user.userId);
    this.adminUserService.deleteUser(user.userId).subscribe({
      next: () => {
        this.busyUsers.delete(user.userId);
        this.users = this.users.filter((u) => u.userId !== user.userId);
      },
      error: (err) => {
        this.busyUsers.delete(user.userId);
        this.errorMessage = this.deleteError(err);
      },
    });
  }

  private replaceUser(updated: UserDto): void {
    this.users = this.users.map((u) =>
      u.userId === updated.userId ? updated : u
    );
  }

  private roleError(err: { status?: number }, roleId: number): string {
    if (err.status === 403 || err.status === 400) {
      return 'The system must keep at least one administrator.';
    }
    return 'Could not change the role. Please try again.';
  }

  private deleteError(err: { status?: number }): string {
    if (err.status === 403) {
      return 'The system must keep at least one administrator.';
    }
    if (err.status === 404) {
      return 'User was not found.';
    }
    return 'Could not delete the user. Please try again.';
  }
}

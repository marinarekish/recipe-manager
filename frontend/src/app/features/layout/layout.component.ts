import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-layout',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.scss',
})

export class LayoutComponent {
  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);

  readonly user = this.authService.currentUser;
  readonly sidebarOpen = signal(false);

  get isAdmin(): boolean {
    return this.user()?.roles?.some((r) => r.name === 'Administrator') ?? false;
  }

  toggleSidebar(): void {
    this.sidebarOpen.update((open) => !open);
  }

  closeSidebar(): void {
    this.sidebarOpen.set(false);
  }

  logout(): void {
    this.authService.logout();
    this.closeSidebar();
    void this.router.navigateByUrl('/login');
  }

  initial(): string {
    const u = this.user();
    const ch = u?.firstName?.[0] ?? u?.email?.[0];
    return ch?.toUpperCase() ?? '?';
  }
}
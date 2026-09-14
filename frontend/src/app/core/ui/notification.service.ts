import { inject, Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private readonly snackbar = inject(MatSnackBar);

  success(message: string) : void {
    this.snackbar.open(
      message, 'Close', {
        duration: 2500,
        horizontalPosition: 'center',
        verticalPosition: 'bottom',
        panelClass: ['snack-success'],
      }
    )
  }

  error(message: string) : void {
    this.snackbar.open(
      message, 'Close', {
        duration: 4000,
        horizontalPosition: 'center',
        verticalPosition: 'bottom',
        panelClass: ['snack-error'],
      }
    )
  }
}

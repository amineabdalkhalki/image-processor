import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Subscription, timer, interval } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { ImageResponse, ImageService } from '../../services/image.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-image-display',
  standalone: true,
  imports: [MatCardModule, DatePipe, MatProgressSpinnerModule],
  templateUrl: './image-display.component.html',
  styleUrl: './image-display.component.scss',
})
export class ImageDisplayComponent implements OnInit, OnDestroy {
  private imageService = inject(ImageService);
  private subscriptions = new Subscription();
  private isRefreshing = false;

  countdown = environment.countdownDuration;
  countdownDuration = environment.countdownDuration;
  imageData: ImageResponse | null = null;
  error: string | null = null;

  ngOnInit() {
    this.setupPolling();
    this.setupCountdown();
  }

  manualRefresh() {
    //block the user from refreshing while the image is being fetched
    if (this.isRefreshing) return;

    this.isRefreshing = true;
    this.countdown = environment.countdownDuration;

    this.imageService.getImageData().subscribe({
      next: (data) => {
        this.imageData = data;
        this.isRefreshing = false;
        this.error = null;
      },
      error: (err) => {
        console.error(err);
        this.isRefreshing = false;
        this.error = 'Failed to load data';
      },
    });
  }

  // Update the subscription to handle errors
  private setupPolling() {
    this.subscriptions.add(
      timer(0, environment.refreshInterval)
        .pipe(
          switchMap(() => {
            this.isRefreshing = true;
            this.error = null; // Reset error state
            return this.imageService.getImageData();
          })
        )
        .subscribe({
          next: (data) => {
            this.imageData = data;
            this.isRefreshing = false;
            this.countdown = environment.countdownDuration;
          },
          error: (err) => {
            this.error = 'Failed to load data'; // Set error message
            this.isRefreshing = false;
          },
        })
    );
  }

  private setupCountdown() {
    this.subscriptions.add(
      interval(1000).subscribe(() => {
        if (!this.isRefreshing) {
          this.countdown = Math.max(0, this.countdown - 1);
        }
      })
    );
  }

  ngOnDestroy() {
    this.subscriptions.unsubscribe();
  }
}

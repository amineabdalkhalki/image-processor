import { Component, inject } from '@angular/core';
import { AsyncPipe, DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { ImageService } from '../../services/image.service';

@Component({
  selector: 'app-image-display',
  standalone: true,
  imports: [MatCardModule, AsyncPipe, DatePipe],
  templateUrl: './image-display.component.html',
  styleUrl: './image-display.component.scss'
})
export class ImageDisplayComponent {
  private imageService = inject(ImageService);
  private readonly interval5Seconds = 5000;

  imageData$ = this.imageService.getLiveData(this.interval5Seconds);
}

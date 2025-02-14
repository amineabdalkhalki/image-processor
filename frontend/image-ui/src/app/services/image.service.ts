import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, interval, switchMap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ImageService {

  private apiUrl = '/api/images';
  constructor(private http: HttpClient) {}

  getImageData(): Observable<ImageResponse> {
    return this.http.get<ImageResponse>(this.apiUrl);
  }

  getLiveData(pollInterval: number): Observable<ImageResponse> {
    return interval(pollInterval).pipe(
      switchMap(() => this.getImageData())
    );
  }
}
export interface ImageEvent {
  imageUrl: string;
  description: string;
  receivedAt?: string;
}

export interface ImageResponse {
  image: ImageEvent | null;
  lastHourCount: number;
}

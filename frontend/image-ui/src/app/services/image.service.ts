import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, interval, switchMap } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ImageService {

  private apiUrl : string = environment.apiUrl;
  constructor(private http: HttpClient) {}

  getImageData(): Observable<ImageResponse> {
    console.log("apiUrl", this.apiUrl);
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

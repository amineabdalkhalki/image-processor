import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ImageService } from './image.service';
import { environment } from '../../environments/environment';

describe('ImageService', () => {
  let service: ImageService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ImageService]
    });
    service = TestBed.inject(ImageService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch image data from API', () => {
    const mockResponse = {
      image: {
        imageUrl: 'https://test.com/image.jpg',
        description: 'Test image',
        receivedAt: '2023-10-01T12:00:00Z'
      },
      lastHourCount: 5
    };

    service.getImageData().subscribe(response => {
      expect(response).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(environment.apiUrl);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should handle API errors', () => {
    const mockError = new ProgressEvent('error');

    service.getImageData().subscribe({
      error: (err) => {
        expect(err).toBeTruthy();
      }
    });

    const req = httpMock.expectOne(environment.apiUrl);
    req.error(mockError);
  });
});

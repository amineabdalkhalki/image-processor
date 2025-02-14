import {
  ComponentFixture,
  TestBed,
  fakeAsync,
  tick,
} from '@angular/core/testing';
import { ImageDisplayComponent } from './image-display.component';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { of, throwError } from 'rxjs';
import { By } from '@angular/platform-browser';
import { ImageService } from '../../services/image.service';

describe('ImageDisplayComponent', () => {
  let component: ImageDisplayComponent;
  let fixture: ComponentFixture<ImageDisplayComponent>;
  let imageService: jasmine.SpyObj<ImageService>;

  const mockData = {
    image: {
      imageUrl: 'https://test.com/image.jpg',
      description: 'Test image',
      receivedAt: '2023-10-01T12:00:00Z',
    },
    lastHourCount: 5,
  };

  beforeEach(async () => {
    const imageServiceSpy = jasmine.createSpyObj('ImageService', [
      'getImageData',
    ]);

    await TestBed.configureTestingModule({
      // Add standalone component to IMPORTS, not declarations
      imports: [ImageDisplayComponent, MatCardModule, MatProgressSpinnerModule],
      providers: [{ provide: ImageService, useValue: imageServiceSpy }],
    }).compileComponents();

    imageService = TestBed.inject(ImageService) as jasmine.SpyObj<ImageService>;
    imageService.getImageData.and.returnValue(of(mockData));

    fixture = TestBed.createComponent(ImageDisplayComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load initial data', fakeAsync(() => {
    fixture.detectChanges();
    tick();

    expect(imageService.getImageData).toHaveBeenCalledTimes(1);
    expect(component.imageData).toEqual(mockData);
  }));

  it('should update countdown every second', fakeAsync(() => {
    fixture.detectChanges();
    tick(1000);

    expect(component.countdown).toBe(4);

    tick(1000);
    expect(component.countdown).toBe(3);
  }));

  it('should refresh data every 5 seconds', fakeAsync(() => {
    fixture.detectChanges();
    tick(5000);

    expect(imageService.getImageData).toHaveBeenCalledTimes(2);
  }));

  it('should handle manual refresh', fakeAsync(() => {
    imageService.getImageData.and.returnValue(of(mockData));

    fixture.detectChanges();
    tick();
    fixture.detectChanges();

    const countdownElement = fixture.debugElement.query(By.css('.countdown'));
    expect(countdownElement).toBeTruthy();

    imageService.getImageData.calls.reset();

    countdownElement.triggerEventHandler('click', null);
    tick();
    fixture.detectChanges();

    expect(imageService.getImageData).toHaveBeenCalledTimes(1);
    expect(component.countdown).toBe(5);
  }));

  it('should show loading spinner initially', () => {
    component.imageData = null;
    fixture.detectChanges();

    const spinner = fixture.debugElement.query(By.css('mat-spinner'));
    expect(spinner).toBeTruthy();
  });

  it('should display error state', fakeAsync(() => {
    const error = new Error('API error');
    imageService.getImageData.and.returnValue(throwError(() => error));

    fixture.detectChanges();
    tick();
    fixture.detectChanges();

    const errorElement = fixture.debugElement.query(By.css('.error-message'));
    expect(errorElement).toBeTruthy();
    expect(errorElement.nativeElement.textContent).toContain(
      'Error loading image data'
    );
  }));
});

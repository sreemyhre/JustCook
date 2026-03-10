import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TagDto, CreateTagDto, UpdateTagDto } from '../models/tag.model';

@Injectable({ providedIn: 'root' })
export class TagService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/tags`;

  getAll(): Observable<TagDto[]> {
    return this.http.get<TagDto[]>(this.base);
  }

  create(dto: CreateTagDto): Observable<TagDto> {
    return this.http.post<TagDto>(this.base, dto);
  }

  update(id: number, dto: UpdateTagDto): Observable<TagDto> {
    return this.http.put<TagDto>(`${this.base}/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}

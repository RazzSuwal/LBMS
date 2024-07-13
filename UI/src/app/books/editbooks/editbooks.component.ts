import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../../shared/services/api.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Book, BookCategory, Booked, Books } from '../../models/models';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'editbooks',
  templateUrl: './editbooks.component.html',
  styleUrl: './editbooks.component.scss'
})
export class EditbooksComponent {
  category: BookCategory[] = [];
  book!: Books;
  bookId!: number;
  editBookFrom: FormGroup;
  old: Books[] = [];
  constructor(
    private fb: FormBuilder,
    private apiService : ApiService,
    private snackBar: MatSnackBar,
    private route: ActivatedRoute
  ) {
    apiService.getCategories().subscribe({
      next: (res: BookCategory[]) => {
        this.category = [];
        res.forEach((b) => this.category.push(b));
      }
    })
    this.editBookFrom = fb.group({
      title: fb.control('', [Validators.required]),
      author: fb.control('', [Validators.required]),
      price: fb.control('', [Validators.required]),
      category: fb.control('', [Validators.required]),
    });
    // apiService.getBookById().subscribe({
    //   next: (res: Books[]) => {
    //     this.old = [];
    //     res.forEach((b) => this.old.push(b));
    //   }
    // })
  }
  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam !== null) {
      this.bookId = +idParam;
      this.getBook();
    } else {
      console.error('ID parameter is null or undefined.');
    }
  }
  getBook(): void {
    this.apiService.getBookById(this.bookId)
      .subscribe(
        (book: Books) => {
          this.book = book;
          console.log('Fetched Book:', book);
        },
        error => {
          console.error('Error fetching book:', error);
          // Handle error as needed (e.g., show error message)
        }
      );
  }
  editBook(){
    let books = {
       id: this.bookId,
       title: this.editBookFrom.get('title')?.value,
       author: this.editBookFrom.get('author')?.value,
       price: this.editBookFrom.get('price')?.value,
       bookCategoryId: this.editBookFrom.get('category')?.value,
       ordered: true,
      bookCategory: null,

    };

   this.apiService.editBook(this.bookId, books)
    .subscribe(
      (result: Booked) => {
        alert('Book updated successfully:');
        this.editBookFrom.reset();
      },
      error => {
        console.error('Error updating book:', error);
      }
    );
   }



}

import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import { Router } from '@angular/router';
import { Constants } from 'src/app/constants';

interface RegisterModel {
  email: string;
  phone: string,
  imageBase64: string,
  password: string
}

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})

export class RegisterComponent implements OnInit {

  registerForm: FormGroup;

  model: RegisterModel;

  imageUrl: string | ArrayBuffer = "https://mdbootstrap.com/img/Photos/Others/placeholder-avatar.jpg";
  file: File;

  constructor(private formBuilder: FormBuilder,
    private http: HttpClient,
    private router: Router) { }

  ngOnInit(): void {

    this.registerForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.pattern("^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$")]],
      phone: ['', [Validators.required, Validators.pattern("[0-9]{10}$")]],
      password: ['', [Validators.required, Validators.minLength(6), Validators.pattern(("(?=.*[a-z])(?=.*[0-9]).{6,}"))]]
    })
  }

  onFileChanged(file: File) {
    if (file) {
      this.file = file;

      const reader = new FileReader();
      reader.readAsDataURL(file);

      reader.onload = event => {
        this.imageUrl = reader.result;
      };
    };
  }

  get _email(){
    return this.registerForm.get('email')
  }

  get _phone(){
    return this.registerForm.get('phone')
  }

  get _password(){
    return this.registerForm.get('password')
  }

  onSubmit() {
  this.model = {
    email: this.registerForm.controls.email.value,
    phone: this.registerForm.controls.phone.value,
    imageBase64: this.imageUrl.toString(),
    password: this.registerForm.controls.password.value
  };
  this.http.post(`${Constants.HOME_URL}/api/Account/register`, this.model).subscribe(data => {
    console.log("Good request", data);
      this.router.navigateByUrl('/');
     },
     badRequest => {
       console.log("Error");
     });
  }
}

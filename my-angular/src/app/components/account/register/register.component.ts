import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import { Constants } from 'src/app/constants';

interface RegisterModel {
  email: string;
}

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})

export class RegisterComponent implements OnInit {

  registerForm: FormGroup;
  model: RegisterModel;

  constructor(private formBuilder: FormBuilder,
    private http: HttpClient) { }

  ngOnInit(): void {

    this.registerForm = this.formBuilder.group({
      email: ['', Validators.required]
    });
  }

  onSubmit(): void {
    console.log("Відправка форми", this.registerForm.controls.email.value);
    this.model = {
      email: this.registerForm.controls.email.value
    };
    this.http.post(`${Constants.HOME_URL}/api/Account/register`, this.model)
      .subscribe(data=>{
        console.log("Good requst", data);
      },
      badRequest => {
        console.log("Error");
      });

    //Constants.HOME_URL

    //;
  }

}

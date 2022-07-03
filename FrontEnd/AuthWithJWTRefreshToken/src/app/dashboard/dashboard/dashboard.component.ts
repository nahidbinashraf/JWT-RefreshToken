import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/shared/auth.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  userInformation = {
    name :'',
    role :''
  }
  constructor(private authService : AuthService) { }

  ngOnInit(): void {
    this.authService.userInformation.subscribe(value => {
        if(value){
          this.userInformation.name = value.name,
          this.userInformation.role = value.role
        }
    })
  }

}

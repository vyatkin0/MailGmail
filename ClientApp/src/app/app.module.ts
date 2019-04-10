import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { MailComponent } from './mail/mail.component';
import { NgModule } from '@angular/core';

@NgModule({
  declarations: [
    MailComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule
  ],
  providers: [],
  bootstrap: [MailComponent]
})
export class AppModule { }

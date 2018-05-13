package com.alansa.ideabag2.views

import android.arch.lifecycle.ViewModelProviders
import android.databinding.DataBindingUtil
import android.os.Bundle
import android.view.View
import android.widget.AdapterView
import com.alansa.ideabag2.BaseActivity
import com.alansa.ideabag2.R
import com.alansa.ideabag2.databinding.ActivitySubmitIdeaBinding
import com.alansa.ideabag2.extensions.using
import com.alansa.ideabag2.utils.Validator
import com.alansa.ideabag2.viewmodels.SubmitIdeaViewModel
import kotlinx.android.synthetic.main.activity_submit_idea.*

class SubmitIdeaActivity : BaseActivity() {
    lateinit var binding: ActivitySubmitIdeaBinding
    lateinit var viewmodel: SubmitIdeaViewModel
    val validator = Validator()
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = DataBindingUtil.setContentView(this, R.layout.activity_submit_idea)
        viewmodel = ViewModelProviders.of(this).get(SubmitIdeaViewModel::class.java)
        binding.viewmodel = viewmodel
        setupToolbar()
        spinner.onItemSelectedListener = object : AdapterView.OnItemSelectedListener {
            override fun onItemSelected(p0: AdapterView<*>?, p1: View?, p2: Int, p3: Long) {
                binding.viewmodel!!.ideaCategory.set(spinner.getItemAtPosition(p2) as String)
            }

            override fun onNothingSelected(p0: AdapterView<*>?) {
            }
        }

        submitBtn.setOnClickListener {
            using(validator) {
                validator.validateIsNotEmpty(textInputLayout2, authorTb.text.toString())
                validator.validateIsNotEmpty(textInputLayout4, ideaTitle.text.toString())
                validator.validateIsNotEmpty(textInputLayout, ideaDescription.text.toString())

                if (validator.passedValidation) {
                    viewmodel.submitIdea(this)
                }
            }
        }
    }
}
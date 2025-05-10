window.startTour = () => {
    const tour = new window.Shepherd.Tour({
        defaultStepOptions: {
            scrollTo: true,
            cancelIcon: {
                enabled: true
            },
            classes: "shepherd-theme-arrows",
            modalOverlayOpeningPadding: 5,
            modalOverlayOpeningRadius: 5,
            arrow: true,
            modalOverlay: true
        }
    });

    tour.addStep({
        id: "welcome",
        text: "Welcome to the Scrum Challenge Dashboard! Let's take a quick tour.",
        buttons: [
            {
                text: "Next",
                action: tour.next
            }
        ],
        styles: {
            popover: {
                backgroundColor: "#3f51b5",
                color: "#fff",
                fontSize: "18px",
                borderRadius: "8px",
                padding: "20px",
                width: "300px",
                maxWidth: "90vw"
            },
            arrow: {
                borderColor: "#3f51b5"
            }
        }
    });

    tour.addStep({
        id: "manage-dev-team",
        text: "Here you can manage your development team. Go here to create developers for your project.",
        attachTo: {
            element: "#manage-dev-team",
            on: "bottom"
        },
        buttons: [
            {
                text: "Next",
                action: tour.next
            }
        ],
        styles: {
            popover: {
                backgroundColor: "#ff5722",
                color: "#fff",
                fontSize: "18px",
                borderRadius: "8px",
                padding: "20px"
            },
            arrow: {
                borderColor: "#ff5722"
            }
        }
    });


    tour.addStep({
        id: "manage-user-stories",
        text: "This section lets you manage user stories. You can assign developers to user stories and view progress.",
        attachTo: {
            element: "#manage-user-stories",
            on: "bottom"
        },
        buttons: [
            {
                text: "Next",
                action: tour.next
            }
        ],
        styles: {
            popover: {
                backgroundColor: "#4caf50",
                color: "#fff",
                fontSize: "18px",
                borderRadius: "8px",
                padding: "20px"
            },
            arrow: {
                borderColor: "#4caf50"
            }
        }
    });

    tour.addStep({
        id: "manage-sprints",
        text: "Here you can manage your sprints, with results for each sprint provided.",
        attachTo: {
            element: "#manage-sprints",
            on: "bottom"
        },
        buttons: [
            {
                text: "Next",
                action: tour.next
            }
        ],
        styles: {
            popover: {
                backgroundColor: "#2196f3",
                color: "#fff",
                fontSize: "18px",
                borderRadius: "8px",
                padding: "20px"
            },
            arrow: {
                borderColor: "#2196f3"
            }
        }
    });

    tour.addStep({
        id: "view-project-summary",
        text: "Once you've completed your sprints, you can review your project's performance here.",
        attachTo: {
            element: "#view-project-summary",
            on: "bottom"
        },
        buttons: [
            {
                text: "Next",
                action: tour.next
            }
        ],
        styles: {
            popover: {
                backgroundColor: "#673ab7",
                color: "#fff",
                fontSize: "18px",
                borderRadius: "8px",
                padding: "20px"
            },
            arrow: {
                borderColor: "#673ab7"
            }
        }
    });

    tour.addStep({
        id: "daily-challenge",
        text: "Here is your Daily Challenge. Accept it to boost engagement and push your project forward!",
        attachTo: {
            element: "#challenge_section_tour",
            on: "bottom"
        },
        buttons: [
            {
                text: "Next",
                action: tour.next
            }
        ],
        styles: {
            popover: {
                backgroundColor: "#009688",
                color: "#fff",
                fontSize: "18px",
                borderRadius: "8px",
                padding: "20px"
            },
            arrow: {
                borderColor: "#009688"
            }
        }
    });

    tour.addStep({
        id: "end",
        text: "Congratulations, you've completed the tour! Ready to start managing your project?",
        buttons: [
            {
                text: "Finish",
                action: tour.complete
            }
        ],
        styles: {
            popover: {
                backgroundColor: "#4caf50",
                color: "#fff",
                fontSize: "18px",
                borderRadius: "8px",
                padding: "20px"
            },
            arrow: {
                borderColor: "#4caf50"
            }
        }
    });

    tour.start();
};